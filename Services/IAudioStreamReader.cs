//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace transcription_project.WebApp.Services
{
    public interface IAudioStreamReader 
    {
        public string GetProperty(PropertyId id);
        public int Read(byte[] dataBuffer, uint size);
        public AudioConfig OpenWavFile(string conversationWaveFile);
        public PullAudioInputStreamCallback OpenWavFileStream(string filename, out AudioStreamFormat format);
    }
}
