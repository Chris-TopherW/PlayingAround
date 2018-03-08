#include "VSTBase.h"

VstBasicParams::VstBasicParams() :
	blocksize(1024),
	samplerate(44100)
{
}

VSTBase::VSTBase(const wchar_t* pluginPath)
{
	loadPlugin(pluginPath);
	configurePluginCallbacks();
}

VSTBase::~VSTBase()
{
	for (int i = 0; i < plugin->numParams; i++)
	{
		free(paramNames[i]);
	}
	free(paramNames);
}

void VSTBase::silenceChannel(float **channelData, int numChannels, long numFrames) {
	for (int channel = 0; channel < numChannels; channel++) {
		for (long frame = 0; frame < numFrames; frame++) {
			channelData[channel][frame] = 0.0f;
		}
	}
}

void VSTBase::loadPlugin(const wchar_t* path) {
	Debug::Log("load plugin called");
	HMODULE modulePtr = LoadLibrary(path);
	//HMODULE modulePtr = LoadLibrary(_T("D:\\UnityProjects\\usingExternalCpp\\VSTHostUnity\\VSTHostUnity\\Reverb.dll"));

	vstPluginFuncPtr mainEntryPoint;
	if (modulePtr) {
		Debug::Log("Module pointer loaded");
		mainEntryPoint = (vstPluginFuncPtr)GetProcAddress(modulePtr, "VSTPluginMain");
		if (!mainEntryPoint)
		{
			Debug::Log("VSTPluginMain is null");
		}
		if (!mainEntryPoint) mainEntryPoint = (vstPluginFuncPtr)GetProcAddress(modulePtr, "VstPluginMain()"); 

		if (!mainEntryPoint) Debug::Log("VSTPluginMain() is null");

		if (!mainEntryPoint) mainEntryPoint = (vstPluginFuncPtr)GetProcAddress(modulePtr, "main"); 
		
		if (!mainEntryPoint) Debug::Log("main is null");
	}
	else
	{
		Debug::Log("C: Failed trying to load VST", Color::Black);
		plugin = NULL;
		return;
	}

	plugin = mainEntryPoint(hostCallback);
	if (plugin == NULL) Debug::Log("Error, falied to instantiate plugin"); 
	else Debug::Log("Plugin instantiated");
	pluginNumInputs = plugin->numInputs;
	pluginNumOutputs = plugin->numOutputs;
	Debug::Log("In out setup");
} 

int VSTBase::configurePluginCallbacks(/*AEffect *plugin*/) {
		// Check plugin's magic number
		// If incorrect, then the file either was not loaded properly, is not a
		// real VST plugin, or is otherwise corrupt.
		if (plugin == NULL)
		{
#ifdef isUnityDLL
			Debug::Log("Error, no plugin");
#elif isConsoleVersion
			cout << "Error, no plugin" << endl;
#endif
			return 0;
		}
		if (plugin->magic != kEffectMagic) {
#ifdef isUnityDLL
			Debug::Log("C: Plugin's magic number is bad\n", Color::Black);
#elif isConsoleVersion
			cout << "C: Plugin's magic number is bad\n" << endl;
#endif
			return -1;
		}

		// Create dispatcher handle
		dispatcherFuncPtr dispatcher = (dispatcherFuncPtr)(plugin->dispatcher);
		if (dispatcher == NULL)
		{
#ifdef isUnityDLL
			Debug::Log("C: dispatcher is NULL\n", Color::Black);
#elif isConsoleVersion
			cout << "C: dispatcher is NULL\n" << endl;
#endif
			
		}

		paramNames = (char**)malloc(sizeof(char*) * plugin->numParams);
		for (int i = 0; i < plugin->numParams; i++)
		{
			paramNames[i] = (char*)malloc(sizeof(char) * 128);
			plugin->dispatcher(plugin, effGetParamName, i, 0, paramNames[i], 0);
		}

		// Set up plugin callback functions
		plugin->getParameter = (getParameterFuncPtr)plugin->getParameter;
		plugin->processReplacing = (processFuncPtr)plugin->processReplacing;
		plugin->setParameter = (setParameterFuncPtr)plugin->setParameter;

		return plugin->magic; //added 2/10. was just plugin...
	}

int VSTBase::getNumParams()
{
	if (plugin == NULL)
	{
#ifdef isUnityDLL
		Debug::Log("Error, no plugin");
#elif isConsoleVersion
		cout << "Error, no plugin" << endl;
#endif
		return 0;
	}
	return plugin->numParams;
}

void VSTBase::setParam(int paramIndex, float p_value)
{
	plugin->setParameter(plugin, paramIndex, p_value);
}

float VSTBase::getParam(int index)
{
	return plugin->getParameter(plugin, index);
}

char* VSTBase::getParamName(int index)
{
	plugin->dispatcher(plugin, effGetParamName, index, 0, paramNames[index], 0);
	return paramNames[index];
}

void VSTBase::startPlugin(/*AEffect *plugin*/) {
	if (plugin == NULL)
	{
#ifdef isUnityDLL
		Debug::Log("Error, no plugin in startPlugin()");
#elif isConsoleVersion
		cout << "Error, no plugin in startPlugin()" << endl;
#endif
		return;
	}
	plugin->dispatcher(plugin, effOpen, 0, 0, NULL, 0.0f);

	// Set some default properties
	float sampleRate = 44100.0f; ////////////////////////////this needs to come from Unity!!!!!! 
	plugin->dispatcher(plugin, effSetSampleRate, 0, 0, NULL, sampleRate);
	plugin->dispatcher(plugin, effSetBlockSize, 0, hostParams->blocksize, NULL, 0.0f);
	//plugin->dispatcher(plugin, effSet)
	resumePlugin(/*plugin*/);
}

void VSTBase::resumePlugin(/*AEffect *plugin*/) {
	if (plugin == NULL)
	{
#ifdef isUnityDLL
		Debug::Log("Error, no plugin in resumePlugin()");
#elif isConsoleVersion
		cout << "Error, no plugin in resumePlugin()" << endl;
#endif
		return;
	}
	plugin->dispatcher(plugin, effMainsChanged, 0, 1, NULL, 0.0f);
}

void VSTBase::suspendPlugin(/*AEffect *plugin*/) {
	if (plugin == NULL)
	{
#ifdef isUnityDLL
		Debug::Log("Error, no plugin in suspendPlugin()");
#elif isConsoleVersion
		cout << "Error, no plugin in suspendPlugin()" << endl;
#endif
		return;
	}
	plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);
}

bool VSTBase::canPluginDo(char *canDoString) {
	if (plugin == NULL)
	{
#ifdef isUnityDLL
		Debug::Log("Error, no plugin in canPluginDo()");
#elif isConsoleVersion
		cout << "Error, no plugin in canPluginDo()" << endl;
#endif
		return false;
	}
	return (plugin->dispatcher(plugin, effCanDo, 0, 0, (void*)canDoString, 0.0f) > 0);
}

void VSTBase::setVstIndex(int p_index)
{
	vstIndex = p_index;
}

extern "C"
{
	VstIntPtr hostCallback(AEffect* effect, VstInt32 opcode, VstInt32 index, VstIntPtr value, void* ptr, float opt) {
		//Debug::Log("Opcode = ");
		//Debug::Log(opcode);
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
}
