#include "VSTi.h"


VSTi::VSTi(const wchar_t* path, VstBasicParams* p_hostParams) : VSTBase(path)
{
	hostParams = p_hostParams;
	initializeIO();
	startPlugin();
}

VSTi::~VSTi()
{
	plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);
	free(multEventsHolder);
	for (int i = 0; i < getNumPluginOutputs(); i++)
	{
		free(pluginOutputs[i]);
	}
	free(pluginOutputs);
}

float* VSTi::processAudio(long numFrames, int numChannels)
{
	return nullptr;
}

void VSTi::setNumInOut()
{ 
	pluginNumOutputs = plugin->numOutputs;
}

void VSTi::initializeIO() {

	//dynamic allocation for varible size struct
	multEventsHolder = (VstEvents*)malloc((sizeof(struct VstEvents) + sizeof(eventHolder)));

	pluginOutputs = (float**)malloc(sizeof(float**) * plugin->numOutputs);
	for (int channel = 0; channel < plugin->numOutputs; channel++) {
		pluginOutputs[channel] = (float*)malloc(sizeof(float) * hostParams->blocksize);
	}
}

int VSTi::getNumPluginOutputs()
{
	if (pluginNumOutputs == -1)
	{
#ifdef isUnityDLL
		Debug::Log("Error, plugin outputs not initialised");
#elif isConsoleVersion
		cout << "Error, plugin outputs not initialised" << endl;
#endif
		return pluginNumOutputs;
	}
	else
	{
		return pluginNumOutputs;
	}
}

void midiEvent(int status, int mess1, int mess2, long delaySamps)
{
	//eventHolder
}