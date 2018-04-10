//Copyright 2018 Chris Wratt and Victoria University of Wellington
//This library is free software : you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.
//
// see <http://www.gnu.org/licenses/> for a full copy of the license.
//Also note that this library CANNOT be used in commercial applications 
//due to restrictions within steinberg's VST license.

using UnityEngine;
using CppDebug;

namespace pluginHost
{
    public static class pluggoHost
    {
        public static int blockSize;
        public static long sampleRate;

        public static void init()
        {
            HostDllCpp.initHost();

            ////////////////////// setup io //////////////////////
            int _numBuff;
            AudioSettings.GetDSPBufferSize(out blockSize, out _numBuff);
            sampleRate = AudioSettings.outputSampleRate;
            HostDllCpp.setHostBlockSize(blockSize);
            HostDllCpp.setSampleRate(sampleRate);
        }
    }

    [ExecuteInEditMode]
    public class Host : MonoBehaviour
    {
        private void Awake()
        {
            pluggoHost.init();
        }
    }
}
