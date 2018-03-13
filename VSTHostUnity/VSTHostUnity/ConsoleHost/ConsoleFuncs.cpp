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

int ConsoleHost::loadEffect(std::string& path)
{
	if (hostInitialised)
	{
		return vstHost->loadEffect(path);
	}
	else
	{
		cout << "Host not initialised, load effect failed" << endl;
		return -1;
	}
}

int ConsoleHost::loadInstrument(std::string& path)
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

float* ConsoleHost::processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels)
{
	return vstHost->getEffect(vstIndex).processAudio(audioThrough, numFrames, numChannels);
}

float* ConsoleHost::processInstAudio(int vstIndex, long numFrames, int numChannels)
{
	return vstHost->getInstrument(vstIndex).processAudio(numFrames, numChannels);
}
