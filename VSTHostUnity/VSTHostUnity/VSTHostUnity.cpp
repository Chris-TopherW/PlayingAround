// VSTHostUnity.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <algorithm>
#include <Objbase.h>
#include <tchar.h>

extern "C" {
	float** inputs;
	float** outputs;
	unsigned int numChannels = 2;
	unsigned int blocksize = 1024;
	char* debugMessage;
	float outputHolder[1024];
	char* pszReturn = NULL;
	//hack in only one plugin for test purposes
	AEffect* plugin = NULL;

	void start()
	{
		debugMessage = (char*)malloc(sizeof(char) * 512);
		debugMessage = "no message";
		//initializeIO();
	}

	void shutdown()
	{
		free(debugMessage);
		free(pszReturn);
		//free(wc);
	}

	float** getOutput()
	{
		return outputs; 
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

	char* cDebugDelegate()
	{
		char* tempDebugMess = debugMessage;
		char* test1 = tempDebugMess;
		size_t stSize = strlen(test1) + sizeof(char);
		pszReturn = NULL;
		//should this be realloc? why am I reallocating every call...
		pszReturn = (char*)::CoTaskMemAlloc(stSize);
		// Copy the contents of test1
		// to the memory pointed to by pszReturn.
		strcpy_s(pszReturn, stSize, test1);

		debugMessage = "no message";
		return pszReturn;
	}


	/*AEffect* */ void loadPlugin(/*char* path*/) {
		//wchar_t* path = L"D:\\UnityProjects\\usingExternalCpp\\Assets\\Data\\Reverb.dll";
		///*AEffect* */plugin = NULL;
		//const WCHAR *vstPath = GetWC(path);

		HMODULE modulePtr = LoadLibrary(_T("C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll"));
		if (modulePtr == NULL) {
			debugMessage = "C: Failed trying to load VST";
			//return NULL;
			plugin = NULL;
			return;
		}

		vstPluginFuncPtr mainEntryPoint =
			(vstPluginFuncPtr)GetProcAddress(modulePtr, "VSTPluginMain");
		// Instantiate the plugin
		plugin = mainEntryPoint(hostCallback); //for some reason this totally breaks in 64 bit build!
		//return plugin;
		return;
	}

	int configurePluginCallbacks(/*AEffect *plugin*/) {
		// Check plugin's magic number
		// If incorrect, then the file either was not loaded properly, is not a
		// real VST plugin, or is otherwise corrupt.
		if (plugin->magic != kEffectMagic) {
			debugMessage = "C: Plugin's magic number is bad\n";
			return -1;
		}

		// Create dispatcher handle
		dispatcherFuncPtr dispatcher = (dispatcherFuncPtr)(plugin->dispatcher);
		if (dispatcher == NULL)
		{
			debugMessage = "C: Dispatcher is NULL\n";
		}

		// Set up plugin callback functions
		plugin->getParameter = (getParameterFuncPtr)plugin->getParameter;
		plugin->processReplacing = (processFuncPtr)plugin->processReplacing;
		plugin->setParameter = (setParameterFuncPtr)plugin->setParameter;

		return plugin->magic; //added 2/10. was just plugin...
	}


	void startPlugin(/*AEffect *plugin*/) {
		plugin->dispatcher(plugin, effOpen, 0, 0, NULL, 0.0f);

		// Set some default properties
		float sampleRate = 44100.0f;
		plugin->dispatcher(plugin, effSetSampleRate, 0, 0, NULL, sampleRate);
		//int blocksize = 512;
		plugin->dispatcher(plugin, effSetBlockSize, 0, blocksize, NULL, 0.0f);

		resumePlugin(/*plugin*/);
	}


	void resumePlugin(/*AEffect *plugin*/) {
		plugin->dispatcher(plugin, effMainsChanged, 0, 1, NULL, 0.0f);
	}

	void suspendPlugin(/*AEffect *plugin*/) {
		plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);
	}


	bool canPluginDo(char *canDoString) {
		return (plugin->dispatcher(plugin, effCanDo, 0, 0, (void*)canDoString, 0.0f) > 0);
	}


	VstIntPtr VSTCALLBACK hostCallback(AEffect *effect, VstInt32 opcode, VstInt32 index,
		VstIntPtr value, void *ptr, float opt) {
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
		// inputs and outputs are assumed to be float** and are declared elsewhere,
		// most likely the are fields owned by this class. numChannels and blocksize
		// are also fields, both should be size_t (or unsigned int, if you prefer).
		//debugMessage = "C: initialiseIo";
		inputs = (float**)malloc(sizeof(float**) * numChannels);
		outputs = (float**)malloc(sizeof(float**) * numChannels);
		for (int channel = 0; channel < numChannels; channel++) {
			inputs[channel] = (float*)malloc(sizeof(float*) * blocksize);
			outputs[channel] = (float*)malloc(sizeof(float*) * blocksize);
		}
	}

	//void processAudio(/*AEffect *plugin,*/ float **inputs, float **outputs,
	//	long numFrames) {
	//	// Always reset the output array before processing.
	//	//silenceChannel(outputs, numChannels, numFrames);

	//	// Note: If you are processing an instrument, you should probably zero
	//	// out the input channels first to avoid any accidental noise. If you
	//	// are processing an effect, you should probably zero the values in the
	//	// output channels. See the silenceChannel() function below.
	//	// However, if you are reading input data from file (or elsewhere), this
	//	// step is not necessary.
	//	//silenceChannel(inputs, numChannels, numFrames);
	//	for (int i = 0; i < numFrames; i++)
	//	{
	//		outputs[0][i] = inputs[0][i] * 0.5f;
	//		outputs[1][i] = inputs[1][i] * 0.5f;
	//		
	//	}

	//	//plugin->processReplacing(plugin, inputs, outputs, numFrames);
	//}
	 
	float* processAudio(float* in, long numFrames)
	{
		//inputs[0] = in;
		//plugin->processReplacing(plugin, inputs, outputs, numFrames);
		inputs[0] = in;
		//for (int i = 0; i < numFrames; i++)
		//{
		//	//outputHolder[i] = in[i];
		//	outputHolder[i] = outputs[0][i];
		//}
		plugin->processReplacing(plugin, inputs, outputs, numFrames);
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
		plugin->dispatcher(plugin, effProcessEvents, 0, 0, events, 0.0f);
	}

	//wchar_t* wc;

	//const wchar_t* GetWC(const char *c)
	//{
	//	const size_t cSize = strlen(c) + 1;
	//	wc = new wchar_t[cSize];
	//	mbstowcs(wc, c, cSize);

	//	return wc;
	//}

}