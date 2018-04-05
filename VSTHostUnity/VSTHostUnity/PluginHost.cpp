#include "PluginHost.h"

int VstHost::loadEffect(std::string& path)
{
	std::shared_ptr<VSTEffect> vstEffect = std::make_shared<VSTEffect>(path, samplerate, blocksize);
	int vstIndex = audioEffects.size();
	
	vstEffect->setVstIndex(vstIndex);
	audioEffects.push_back(vstEffect);
	Debug::Log("audio effects array size = ");
	Debug::Log((int)audioEffects.size());
	return vstIndex;
}

int VstHost::loadInstrument(std::string& path)
{
	std::shared_ptr<VSTi> vstInst = std::make_shared<VSTi>(path, samplerate, blocksize);
	int vstIndex = audioEffects.size();
	vstInst->setVstIndex(vstIndex);
	instruments.push_back(vstInst);
	return vstIndex;
}

void VstHost::setBlockSize(int p_blocksize)
{
	blocksize = p_blocksize;
}

std::shared_ptr<VSTEffect> VstHost::getEffect(int vstIndex)
{
	if (audioEffects.size() >= vstIndex)
	{
		return audioEffects[vstIndex];
	}
	else
	{
		Debug::Log("Error: trying to access non-existent audio effect");
		return audioEffects[vstIndex];
	}
}

std::shared_ptr<VSTi> VstHost::getInstrument(int index)
{
	if (instruments.size() >= index)
	{
		return instruments[index];
	}
	else
	{
	Debug::Log("Error: trying to access non-existent vst instrument");
		return instruments[index];
	}
}

void VstHost::clearVSTs()
{
	if(audioEffects.size() > 0)
		audioEffects.clear();
	if(instruments.size() > 0)
		instruments.clear();
}
