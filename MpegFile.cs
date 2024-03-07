﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace MpegMetadata;
internal class MpegFile(string filePath) {

    // don't change casing
    private readonly JsonSerializerOptions myJsonOptions = new() { PropertyNamingPolicy = null};
    
    public void Process() {

        var rawData = ReadData();

        var noMarks = RemoveFrameMarks(rawData);

        var noPadding = RemovePadding(noMarks);

        var json = System.Text.Encoding.ASCII.GetString(noPadding);

        var info = Deserialise(json);

        info.ChannelAffiliate = "Blah"; // make a change!

        json = Serialise(info);

        var restored = ReplaceFrameMarks(ReplacePadding(Encoding.ASCII.GetBytes(json)));

        //for (var i = 0; i < rawData.Length; i++) {
        //    if (rawData[i] != restored[i]) {
        //        Console.WriteLine($"{i} - Different: {rawData[i]:X} -> {restored[i]:X}");
        //    }
        //}

        WriteBack(restored);
    }

    private string Serialise(ProgrammeInformation info) {
        return JsonSerializer.Serialize(info, myJsonOptions);
    }

    private ProgrammeInformation Deserialise(string json) {
        return JsonSerializer.Deserialize<ProgrammeInformation>(json, myJsonOptions) ?? new ProgrammeInformation();
    }

    private byte[] RemoveFrameMarks(byte[] input) {
        var output = new byte[64 * 184];
        var j = 0;
        for (var i = 0; i < input.Length; i++) {
            var frameOffset = i % 188;
            if (frameOffset > 3) output[j++] = input[i];
            // else if (frameOffset == 0) Console.WriteLine($"Frame | {input[i]:X} | {input[i + 1]:X} | {input[i + 2]:X} | {input[i+3]:X}");
        }
        return output;
    }

    // my best go at reverse engineering the TS frame delimiters...
    private byte[] ReplaceFrameMarks(byte[] input) {
        var output = new byte[64 * 188];
        var j = 0;
        output[j++] = 0x47;
        output[j++] = 0x5F;
        output[j++] = 0xFA;
        output[j++] = 0x10;
        for (var i = 0; i < input.Length; i++) {
            if (i % 184 == 0 && i > 0) {
                var seq = ((i / 184) % 16) + 0x10; // cycles through 0x10 -> 0x1F
                //Console.WriteLine($"Seq = {seq} = {(seq + 0x10):X}");
                output[j++] = 0x47;
                output[j++] = 0x1F;
                output[j++] = 0xFA;
                output[j++] = (byte)seq;
            }
            output[j++] = input[i];
        }
        return output;
    }


    private byte[] RemovePadding(byte[] input) {
        var len = input.Length;
        if (len != 11776) throw new ArgumentException($"Expected length to be 11776: {len}");
        var pos = len - 1;
        for (; pos >= 0; pos--) {
            if (input[pos] != 0xFF) break;
        }
        pos++;
        var output = new byte[pos];
        for (var i = 0; i < pos; i++) {
            output[i] = input[i];
        }
        return output;
    }

    private byte[] ReplacePadding(byte[] input) {
        var output = new byte[11776];
        var i = 0;
        for (; i < input.Length && i < 11776; i++) {
            output[i] = input[i];
        }
        for (; i < 11776; i++) {
            output[i] = 0xFF;
        }
        return output;
    }


    private byte[] ReadData() {
        var buffer = new byte[12032];
        using var stream = File.OpenRead(filePath);
        var offset = 0;
        var remaining = buffer.Length;
        while (remaining > 0) {
            var didReadBytes = stream.Read(buffer, offset, remaining);
            if (didReadBytes <= 0) throw new EndOfStreamException($"End of stream reached with {remaining} bytes left to read");
            remaining -= didReadBytes;
            offset += didReadBytes;
        }
        return buffer;
    }

    private void WriteBack(byte[] data) {
        using var stream = File.Open(filePath, FileMode.Open);
        stream.Position = 0;
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
    }
}

