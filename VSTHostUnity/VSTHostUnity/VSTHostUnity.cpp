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
		plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);
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

		HMODULE modulePtr = LoadLibrary(_T("C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll"));
		//HMODULE modulePtr = LoadLibrary(_T("D:\\UnityProjects\\usingExternalCpp\\VSTHostUnity\\VSTHostUnity\\Reverb.dll"));

		vstPluginFuncPtr mainEntryPoint;
		if (modulePtr) {
			mainEntryPoint = (vstPluginFuncPtr)GetProcAddress(modulePtr, "VSTPluginMain");
			if (!mainEntryPoint)
			{
				Debug::Log("VSTPluginMain is null");
			}
			if (!mainEntryPoint) {
				mainEntryPoint = (vstPluginFuncPtr)GetProcAddress(modulePtr, "VstPluginMain()");
			}
			if (!mainEntryPoint)
			{
				Debug::Log("VstPluginMain() is null");
			}
			if (!mainEntryPoint) {
				mainEntryPoint = (vstPluginFuncPtr)GetProcAddress(modulePtr, "main");
			}
			if (!mainEntryPoint)
			{
				Debug::Log("main is null");
			}
		}
		else
		{
			Debug::Log("C: Failed trying to load VST", Color::Black);
			//return NULL;
			plugin = NULL;
			return;
		}

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
		//plugin->dispatcher(plugin, effSet)
		resumePlugin(/*plugin*/);


		silenceChannel(inputs, numChannels, blocksize);
		silenceChannel(outputs, numChannels, blocksize);


		Debug::Log("About to get into processBlock");
		Debug::Log("inputs ptr size: ");
		Debug::Log((int)sizeof(inputs));
		Debug::Log("inputs[0] size: ");
		Debug::Log((int)sizeof(inputs[0]));
		Debug::Log("input[1] size: ");
		Debug::Log((int)sizeof(inputs[1]));
		Debug::Log("outputs ptr size: ");
		Debug::Log((int)sizeof(outputs));
		Debug::Log("outputs[0] size: ");
		Debug::Log((int)sizeof(outputs[0]));
		Debug::Log("outputs[1] size: ");
		Debug::Log((int)sizeof(outputs[1]));
		Debug::Log("blocksize: ");
		Debug::Log((int)blocksize);
		Debug::Log("Plugin pointer location lolol: ");
		Debug::Log((int)plugin);
	}

	void processBlock()
	{
		//plugin->processReplacing(plugin, inputs, outputs, blocksize);
		//cout << "Running process block. Out pos 0 is: " << outputs[0][0] << endl;

		Debug::Log("Num inputs in plugin = ");
		Debug::Log(plugin->numInputs);
		Debug::Log("Num outputs in plugin = ");
		Debug::Log(plugin->numOutputs);

		float** testArray = (float**)malloc(sizeof(float*) * plugin->numInputs);
		float** testOutArray = (float**)malloc(sizeof(float*) * plugin->numOutputs);
		int block = 1024;
		for (int i = 0; i < plugin->numInputs; i++)
		{
			testArray[i] = (float*)malloc(sizeof(float) * blocksize);
		}
		for (int i = 0; i < plugin->numOutputs; i++)
		{
			testOutArray[i] = (float*)malloc(sizeof(float) * blocksize);
		}

		for (int chan = 0; chan < plugin->numInputs; chan++)
		{
			for (int i = 0; i < block; i++)
			{
				testArray[chan][i] = 0.0f;
			}
		}
		for (int chan = 0; chan < plugin->numOutputs; chan++)
		{
			for (int i = 0; i < block; i++)
			{
				testOutArray[chan][i] = 0.0f;
			}
		}
		
		plugin->processReplacing(plugin, testArray, testOutArray, block);
		if (testOutArray != NULL)
		{
			Debug::Log("Running process block. Out pos 0 is: ");
		}
		else
		{
			Debug::Log("testOutArray is NULL");
		}

		free(testArray[0]);
		free(testOutArray[0]);
		free(testArray[1]);
		free(testOutArray[1]);
		free(testArray);
		free(testOutArray);
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
		Debug::Log("Opcode = ");
		Debug::Log(opcode);
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
		inputs = (float**)malloc(sizeof(float**) * numChannels); //2*8+(1024*4*2)
		outputs = (float**)malloc(sizeof(float**) * numChannels);
		for (int channel = 0; channel < numChannels; channel++) {
			inputs[channel] = (float*)malloc(sizeof(float) * blocksize);
			outputs[channel] = (float*)malloc(sizeof(float) * blocksize);
		}
	}
	
	float* processAudio(float* in, long numFrames)
	{
		silenceChannel(inputs, numChannels, numFrames);
		silenceChannel(outputs, numChannels, numFrames);
		//inputs[0] = in;
		outputs[0] = inputs[0];
		
		//plugin->processReplacing(plugin, inputs, outputs, numFrames);
		//plugin->__processDeprecated(plugin, inputs, outputs, numFrames);
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
