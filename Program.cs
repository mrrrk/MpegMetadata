// See https://aka.ms/new-console-template for more information


using MpegMetadata;

var file = new MpegFile("D:\\Recorded TV\\Mallorca Files\\Mallorca Files - S01E01 - Death in the Morning.mpg");

file.Process();

Console.WriteLine("Done");
