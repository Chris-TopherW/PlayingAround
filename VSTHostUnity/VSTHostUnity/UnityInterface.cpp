#include "UnityInterface.h"

void initHost()
{
	if (!hostInitialised)
	{
		vstHost = std::make_shared<VstHost>();
		hostInitialised = true;
	}
	else
	{
		Debug::Log("Error: host already initialised, exiting initHost()");
	}
}

void setSampleRate(long p_sampleRate)
{
	if (hostInitialised)
		vstHost->samplerate = p_sampleRate;
	else
		Debug::Log("Host not initialised, set sr failed");
}

void setHostBlockSize(int p_blocksize)
{
	if (hostInitialised)
		vstHost->blocksize = p_blocksize;
	else
		Debug::Log("Host not initialised, set blocksize failed");
}

int loadEffect(char* path)
{
	if (hostInitialised)
	{
		//hack in const location for testing
		path = "C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
		return vstHost->loadEffect(path);
	}
	else
	{
		Debug::Log("Host not initialised, load effect failed");
		return -1;
	}
}

int loadInstrument(char* path)
{
	if (hostInitialised)
	{
		return vstHost->loadInstrument(path);
	}
	else
	{
		Debug::Log("Host not initialised, load instr failed");
		return -1;
	}
	
}

//not going to do init checks in these as they get called too much
float* processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels)
{
	return vstHost->getEffect(vstIndex).processAudio(audioThrough, numFrames, numChannels);
}

float* processInstAudio(int vstIndex, long numFrames, int numChannels)
{
	return vstHost->getInstrument(vstIndex).processAudio(numFrames, numChannels);
}

