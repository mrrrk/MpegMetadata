# MpegMetadata

For editing metadata of **HDHomeRun** programme (.mpg) file.

The first 12032 bytes appear to be given over to 64 x 188 byte frames which contain JSON formatted metadata.

Each frame starts with a four-byte delimter and these are slotted into the JSON, so we need to:

   - strip out the frame delimiters
   - strip off the `0xFF` padding characters
   - Convert to string (ASCII? UTF8?)
   - deserialise JSON


Assembly is reverse of disassembly!

The frame delimiters seem to incorporate a sequence counter in fourth byte (last 4 bits?) that starts at `0x10`, counts up to `0x1F` and repeats.
