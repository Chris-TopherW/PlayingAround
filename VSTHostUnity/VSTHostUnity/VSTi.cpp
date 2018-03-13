#include "VSTi.h"

VSTi::VSTi(std::string path, VstBasicParams& p_hostParams) : VSTBase(path)
{
	hostParams = p_hostParams;
	initializeIO();
	startPlugin();
}

VSTi::~VSTi()
{
	plugin->dispatcher(plugin, effMainsChanged, 0, 0, NULL, 0.0f);
	free(multEventsHolder);
}

float* VSTi::processAudio(long numFrames, int numChannels)
{
	silenceChannel(pluginOutputs);
	return nullptr;
}

void VSTi::setNumInOut()
{ 
	pluginNumOutputs = plugin->numOutputs;
}

void VSTi::initializeIO() {

	std::vector<float> tempVect;
	tempVect.reserve(hostParams.blocksize);
	for (int i = 0; i < hostParams.blocksize; i++)
	{
		tempVect.push_back(0.0f);
	}
	for (int i = 0; i < plugin->numOutputs; i++)
	{
		pluginOutputs.push_back(tempVect);
	}

	//hold locations of starts of i/o vectors as raw float** in order to access for plugin.processReplacing
	std::vector<float*> target2(pluginOutputs.size());
	for (int i = 0; i < pluginOutputs.size(); i++)
	{
		target2[i] = &*pluginOutputs[i].begin();
	}
	pluginOutputsStartPtr = (float**)malloc((sizeof(float*) * 2));
	pluginOutputsStartPtr[0] = target2[0];
	pluginOutputsStartPtr[1] = target2[1];
}

int VSTi::getNumPluginOutputs()
{
	if (pluginNumOutputs == -1)
	{
		Debug::Log("Error, plugin outputs not initialised");
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