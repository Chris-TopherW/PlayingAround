// VSTHostUnity.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <algorithm>
#include <Objbase.h>
#include <tchar.h>
#include "DebugCPP.h"

extern "C" {
	float** inputs;
	float** outputs;
	unsigned int numChannels = 2;
	unsigned int blocksize = 1024;
	float outputHolder[1024];
	char* pszReturn = NULL;
	wchar_t* wc;
	//hack in only one plugin for test purposes
	AEffect* plugin = NULL;

	void start()
	{
	}

	void shutdown()
	{
		free(pszReturn);
		free(wc);
		for (int i = 0; i < numChannels; i++)
		{
			free(inputs[i]);
			free(outputs[i]);
		}
		free(inputs);
		free(outputs);
	}

	void setBlockSize(int p_blocksize)
	{
		blocksize = p_blocksize;
		//debugMessage = "Block";
	}
	void setNumChannels(int p_numChannels)
	{
		numChannels = p_numChannels;
	}

	/*AEffect* */ void loadPlugin(/*char* path*/) {
		//wchar_t* path = L"D:\\UnityProjects\\usingExternalCpp\\Assets\\Data\\Reverb.dll";
		///*AEffect* */plugin = NULL;
		//const WCHAR *vstPath = GetWC(path);
		 
		//need to stop application from crashing if null
		char* pluginPath = "C:\\Program Files\\Steinberg\\VSTPlugins\\SpaceshipDelay.dll";
		HMODULE modulePtr = LoadLibrary(GetWC(pluginPath));
		//HMODULE modulePtr = LoadLibrary(_T("C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\brandulator.dll"));
		
		if (modulePtr == NULL) {
			Debug::Log("C: Failed trying to load VST", Color::Black);
			//return NULL;
			plugin = NULL;
			return;
		}

		vstPluginFuncPtr mainEntryPoint =
			(vstPluginFuncPtr)GetProcAddress(modulePtr, "VSTPluginMain");
		// Instantiate the plugin
		//need to look deep into host callback function- I think this is where the error is coming from...
		plugin = mainEntryPoint(hostCallback); //for some reason this totally breaks in 64 bit build!
		if (plugin == NULL)
		{
			Debug::Log("Error, falied to instantiate plugin");
		}
		//return plugin;
		return;
	}

	int configurePluginCallbacks(/*AEffect *plugin*/) {
		// Check plugin's magic number
		// If incorrect, then the file either was not loaded properly, is not a
		// real VST plugin, or is otherwise corrupt.
		if (plugin == NULL)
		{
			Debug::Log("Error, no plugin");
			return 0;
		}
		if (plugin->magic != kEffectMagic) {
			Debug::Log("C: Plugin's magic number is bad\n", Color::Black);
			return -1;
		}

		// Create dispatcher handle
		dispatcherFuncPtr dispatcher = (dispatcherFuncPtr)(plugin->dispatcher);
		if (dispatcher == NULL)
		{
			Debug::Log("C: dispatcher is NULL\n", Color::Black);
		}

		// Set up plugin callback functions
		plugin->getParameter = (getParameterFuncPtr)plugin->getParameter;
		plugin->processReplacing = (processFuncPtr)plugin->processReplacing;
		plugin->setParameter = (setParameterFuncPtr)plugin->setParameter;

		return plugin->magic; //added 2/10. was just plugin...
	}

	int getNumParams()
	{
		if (plugin == NULL)
		{
			Debug::Log("Error, no plugin");
			return 0;
		}
		return plugin->numParams;
	}

	void setParam(int paramIndex, float p_value)
	{
		plugin->setParameter(plugin, paramIndex, p_value);
	}

	void startPlugin(/*AEffect *plugin*/) {
		if (plugin == NULL)
		{
			Debug::Log("Error, no plugin in startPlugin()");
			return;
		}
		plugin->dispatcher(plugin, effOpen, 0, 0, NULL, 0.0f);

		// Set some default properties
		float sampleRate = 44100.0f;
		plugin->dispatcher(plugin, effSetSampleRate, 0, 0, NULL, sampleRate);
		//int blocksize = 512;
		plugin->dispatcher(plugin, effSetBlockSize, 0, blocksize, NULL, 0.0f);

		resumePlugin(/*plugin*/);
	}

	void resumePlugin(/*AEffect *plugin*/) {
		if (plugin == NULL)
		{
			Debug::Log("Error, no plugin in resumePlugin()");
			return;
		}
		plugin->dispatcher(plugin, effMainsChanged, 0, 1, NULL, 0.0f);
	}

	void suspendPlugin(/*AEffect *plugin*/) {
		if (plugin == NULL)
		{
			Debug::Log("Error, no plugin in suspendPlugin()");
			return;
		}
		plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);
	}

	bool canPluginDo(char *canDoString) {
		if (plugin == NULL)
		{
			Debug::Log("Error, no plugin in canPluginDo()");
			return false;
		}
		return (plugin->dispatcher(plugin, effCanDo, 0, 0, (void*)canDoString, 0.0f) > 0);
	}

	VstIntPtr VSTCALLBACK hostCallback(AEffect *effect, VstInt32 opcode, VstInt32 index,
		VstIntPtr value, void *ptr, float opt) {
		if (plugin == NULL)
		{
			Debug::Log("Error, no plugin in hostCallback");
			return NULL;
		}
		switch (opcode) {
		case audioMasterVersion:
			return 2400;
		case audioMasterIdle:
			effect->dispatcher(effect, effEditIdle, 0, 0, 0, 0);
			// Handle other opcodes here... there will be lots of them
		default:
			//debugMessage = "C: Plugin requested value of opcode %d\n";
			break;
		}
	}
	 
	void initializeIO() {
		inputs = (float**)malloc(sizeof(float**) * numChannels);
		outputs = (float**)malloc(sizeof(float**) * numChannels);
		for (int channel = 0; channel < numChannels; channel++) {
			inputs[channel] = (float*)malloc(sizeof(float*) * blocksize);
			outputs[channel] = (float*)malloc(sizeof(float*) * blocksize);
		}
	}
	 
	float* processAudio(float* in, long numFrames)
	{
		if (plugin == NULL) return NULL; 
		//inputs[0] = in;
		//plugin->processReplacing(plugin, inputs, outputs, numFrames);
		inputs[0] = in;
		for (int i = 0; i < numFrames; i++)
		{
			inputs[0][i] = in[i] * 0.5f;
		}
		plugin->processReplacing(plugin, inputs, outputs, numFrames);
		Debug::Log("Audio output at start of buffer = ");
		Debug::Log(outputs[0][0]);
		return outputs[0];
	}

	void silenceChannel(float **channelData, int numChannels, long numFrames) {
		for (int channel = 0; channel < numChannels; ++channel) {
			for (long frame = 0; frame < numFrames; ++frame) {
				channelData[channel][frame] = 0.0f;
			}
		}
	}

	void processMidi(/*AEffect *plugin, */VstEvents *events) {
		if (plugin == NULL) return;
		plugin->dispatcher(plugin, effProcessEvents, 0, 0, events, 0.0f);
	}

	const wchar_t* GetWC(const char *c)
	{
		const size_t cSize = strlen(c) + 1;
		if (wc != nullptr)
		{
			delete(wc);
		}
		
		wc = new wchar_t[cSize];
		mbstowcs(wc, c, cSize);

		return wc;
	}
}
