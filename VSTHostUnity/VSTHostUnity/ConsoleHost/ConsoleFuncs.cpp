#include "ConsoleFuncs.h"
#include <iostream>

using namespace std;

void ConsoleHost::initHost()
{
	if (!hostInitialised)
	{
		vstHost = std::make_shared<VstHost>();
		hostInitialised = true;
	}
	else
	{
		cout << "Error: host already initialised, exiting initHost()" << endl;
	}
}

void ConsoleHost::setSampleRate(long p_sampleRate)
{
	if (hostInitialised)
		vstHost->samplerate = p_sampleRate;
	else
		cout << "Host not initialised, set sr failed" << endl;
}

void ConsoleHost::setHostBlockSize(int p_blocksize)
{
	if (hostInitialised)
		vstHost->blocksize = p_blocksize;
	else
		cout << "Host not initialised, set blocksize failed" << endl;
}

int ConsoleHost::loadEffect(char* path)
{
	if (hostInitialised)
	{
		//hack in const location for testing
		path = "C:\\Users\\chriswratt\\Documents\\UnityProjects\\UnityMidiLib\\VSTHostUnity\\VSTHostUnity\\TAL-Reverb-2.dll";
		return vstHost->loadEffect(path);
	}
	else
	{
		cout << "Host not initialised, load effect failed" << endl;
		return -1;
	}
}

int ConsoleHost::loadInstrument(char* path)
{
	if (hostInitialised)
	{
		return vstHost->loadInstrument(path);
	}
	else
	{
		cout << "Host not initialised, load instr failed" << endl;
		return -1;
	}

}

//not going to do init checks in these as they get called too much
float* ConsoleHost::processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels)
{
	return vstHost->getEffect(vstIndex).processAudio(audioThrough, numFrames, numChannels);
}

float* ConsoleHost::processInstAudio(int vstIndex, long numFrames, int numChannels)
{
	return vstHost->getInstrument(vstIndex).processAudio(numFrames, numChannels);
}
