//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using Serilog;
using Microsoft.EntityFrameworkCore;
namespace transcription_project.WebApp.Utils
{


    public static class HandleException
    {
        public static void Handler(Exception e)
        {
            switch (e)
            {
                case System.Net.Http.HttpRequestException:
                    Log.Error("connection was forcibly closed by the remote host");
                    break;

                case DbUpdateException:
                    var ex = (DbUpdateException)e;
                    // Detach all entries that caused exception
                    foreach (var entry in ex.Entries)
                    {
                        entry.State = EntityState.Detached;
                    }
                    break;

                default:
                    Log.Error($"Unable to handle exception. {e.Message}");
                    break;
            }
        }
    }
}