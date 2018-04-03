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

int setSampleRate(long p_sampleRate)
{
	if (hostInitialised)
	{
		vstHost->samplerate = p_sampleRate;
		return 1;
	}
	else
	{
		Debug::Log("Host not initialised, set sr failed");
		return 0;
	}
}

int setHostBlockSize(int p_blocksize)
{
	if (hostInitialised)
	{
		vstHost->blocksize = p_blocksize;
		return 1;
	}
	else
	{
		Debug::Log("Host not initialised, set blocksize failed");
		return 0;
	}
}

int loadEffect(const char* path)
{
	if (hostInitialised)
	{
		//char * testPath = "C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
		std::string effectLocation(path);
		Debug::Log("Effect location is: ");
		Debug::Log(effectLocation);
		//std::string effectLocation(testPath);
		int effectIndex = vstHost->loadEffect(effectLocation);
		//error catcher
		if (vstHost->getEffect(effectIndex)->getPluginReady() != true) return -1;
			
		return effectIndex;
	}
	else
	{
		Debug::Log("Host not initialised, load effect failed");
		return -1;
	}
}

int loadInstrument(const char* path)
{
	if (hostInitialised)
	{
		//const char * testPath = "C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
		std::string effectLocation(path);
		int instIndex = vstHost->loadInstrument(effectLocation);
		//error catcher
		if (vstHost->getInstrument(instIndex)->getPluginReady() != true) return -1;

		return instIndex;
	}
	else
	{
		Debug::Log("Host not initialised, load instr failed");
		return -1;
	}
}

float* processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels)
{
	//return audioThrough;
	return vstHost->getEffect(vstIndex)->processAudio(audioThrough, numFrames, numChannels);
}

float* processInstAudio(int vstIndex, long numFrames, int numChannels)
{
	return vstHost->getInstrument(vstIndex)->processAudio(numFrames, numChannels);
}

int getNumPluginInputs(int vstIndex)
{
	return vstHost->getEffect(vstIndex)->getNumPluginInputs();
}

int getNumPluginOutputs(int vstIndex)
{
	return vstHost->getEffect(vstIndex)->getNumPluginOutputs();
}

//////////////////////////////////////Parameters/////////////////////////////////////////////////////////

int getNumParams(int vstIndex)
{
	return vstHost->getEffect(vstIndex)->getNumParams();
}

float getParam(int vstIndex, int paramIndex)
{
	return vstHost->getEffect(vstIndex)->getParam(paramIndex);
}

char* getParamName(int vstIndex, int paramIndex)
{
	const char *cstr = vstHost->getEffect(vstIndex)->getParamName(paramIndex).c_str();
	return const_cast<char*>(cstr); //this could be dangerous...?
}

int setParam(int vstIndex, int paramIndex, float p_value)
{
	if (hostInitialised)
	{
		vstHost->getEffect(vstIndex)->setParam(paramIndex, p_value);
		return 1;
	}
	else return 0;
}


///////////////////////////////////Debug////////////////////////////////////////////

int getEffectVectorSize()
{
	return vstHost->getAudioFxVecSize();
}

int getHostNumRef()
{
	return vstHost.use_count();
}
