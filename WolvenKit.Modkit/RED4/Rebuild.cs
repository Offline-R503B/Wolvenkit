using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Catel;
using WolvenKit.Common;
using WolvenKit.Common.Services;
using WolvenKit.Modkit.RED4;
using WolvenKit.RED4.CR2W;

namespace CP77.CR2W
{
    /// <summary>
    /// Collection of common modding utilities.
    /// </summary>
    public partial class ModTools
    {
        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="infolder"></param>
        /// <param name="useBuffers"></param>
        /// <param name="useTextures"></param>
        /// <param name="import"></param>
        /// <param name="keep"></param>
        /// <param name="clean"></param>
        /// <param name="unsaferaw"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public bool Recombine(DirectoryInfo infolder,
            bool useBuffers,
            bool useTextures,
            bool import,    //create new
            bool keep,
            bool clean,
            bool unsaferaw
        )
        {
            var allFiles = infolder.GetFiles("*", SearchOption.AllDirectories).ToList();
            var buffersDict = new Dictionary<string, List<FileInfo>>();

            // get all buffers and textures to recombine
            // if both buffers and textures is selected and a file has both buffers and textures
            // then the texture has priority
            if (useBuffers)
                GetBuffers();
            if (useTextures)
                GetTextures();
            _loggerService.Info($"Found {buffersDict.Count.ToString()} file(s) to rebuild.");

            Thread.Sleep(1000);
            var progress = 0;
            _progressService.Report(0);

            // loop through the buffersDict
            foreach (var (parentPath, buffers) in buffersDict)
            {
                var ext = Path.GetExtension(parentPath)[1..];
                var canImport = Enum.GetNames(typeof(ECookedFileFormat)).Any(_ => _ == ext);

                // if the parent cr2w exists
                if (File.Exists(parentPath))
                {
                    AppendBuffersToFile(parentPath, buffers);
                }
                // if no parent exists then import the raw file into redengine
                else if (import && canImport)
                {
                    //TODO: switch and call import
                    _loggerService.Error("Importing raw files into redengine files is not yet implemented.");
                }

                // if clean is selected, delete the buffer files
                if (clean)
                {
                    //TODO: loop and delet buffers
                    _loggerService.Error("Deleting raw files after rebuilding is not yet implemented.");
                }

                Interlocked.Increment(ref progress);
                _progressService.Report(progress / (float)buffersDict.Count);
            }

            _loggerService.Success($"Successfully rebuilt {buffersDict.Count.ToString()} file(s).");
            return true;

            #region local methods

            void GetBuffers()
            {
                var buffersList = allFiles.Where(_ => _.Extension.ToLower() == ".buffer");
                foreach (var fileInfo in buffersList)
                {
                    // buffer path e.g. stand__rh_hold_tray__serve_milkshakes__01.scenerid.11.buffer
                    // removes 7 characters (".buffer") and then removes the variable length extension (".11")
                    var relpath = fileInfo.FullName;//[(infolder.FullName.Length + 1)..];
                    var parentpath = Path.ChangeExtension(relpath.Remove(relpath.Length - 7), "").TrimEnd('.');
                    var key = parentpath;//FNV1A64HashAlgorithm.HashString(parentpath);

                    // add buffer
                    if (!buffersDict.ContainsKey(key))
                        buffersDict.Add(key, new List<FileInfo>());
                    buffersDict[key].Add(fileInfo);
                }
            }

            void GetTextures()
            {
                if (!import && !unsaferaw)
                    return;

                var texturesList = allFiles.Where(_ => _.Extension.ToLower() == ".dds");
                foreach (var fileInfo in texturesList)
                {
                    // dds path e.g. stand__rh_hold_tray__serve_milkshakes__01.dds
                    // rename to xbm and hash
                    var relpath = fileInfo.FullName;//[(infolder.FullName.Length + 1)..];
                    var parentpath = Path.ChangeExtension(relpath, "xbm");
                    var key = parentpath;//FNV1A64HashAlgorithm.HashString(parentpath);

                    // add buffer
                    if (!buffersDict.ContainsKey(key))
                        buffersDict.Add(key, new List<FileInfo>());
                    else
                        // there can always only be one texture and it get's priority
                        buffersDict[key] = new List<FileInfo>();
                    buffersDict[key].Add(fileInfo);
                }
            }

            bool AppendBuffersToFile(string parentPath, List<FileInfo> buffers_in)
            {
                //check if cr2w
                using var fileStream = new FileStream(parentPath, FileMode.Open, FileAccess.ReadWrite);
                using var fileReader = new BinaryReader(fileStream);

                var cr2w = _wolvenkitFileService.TryReadRED4FileHeaders(fileReader);
                if (cr2w == null)
                {
                    _loggerService.Error($"Failed to read cr2w file {parentPath}");
                    return false;
                }

                // sort buffers numerically
                var buffers = buffers_in;
                if (buffers_in.All(_ => _.Extension == ".buffer"))
                {
                    buffers = buffers_in
                        .OrderBy(_ =>
                            int.Parse(Path.GetExtension(_.FullName.Remove(_.FullName.Length - 7))[1..]))
                        .ToList();
                }

                if (keep)
                {
                    // remove old buffers
                    fileReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    fileStream.SetLength(cr2w.Header.objectsEnd);

                    // kraken the buffers and handle textures
                    using var fileWriter = new BinaryWriter(fileStream);
                    fileWriter.BaseStream.Seek(0, SeekOrigin.End);

                    var existingBufferCount = cr2w.Buffers.Count;

                    for (var i = 0; i < buffers.Count; i++)
                    {
                        var buffer = buffers[i];
                        if (!buffer.Exists)
                            throw new FileNotFoundException($"Could not find file {buffer.FullName} anymore.");

                        var inbuffer = ReadBuffer(buffer);
                        if (inbuffer.Length < 1)
                            continue;

                        var offset = (uint)fileWriter.BaseStream.Position;
                        var (zsize, crc) = fileWriter.CompressAndWrite(inbuffer);

                        if (i < existingBufferCount)
                        {
                            var b = cr2w.Buffers[i];
                            b.Offset = offset;
                            b.DiskSize = zsize;
                            b.MemSize = (uint)inbuffer.Length;
                            b.Crc32 = crc;
                        }
                        else
                        {
                            cr2w.Buffers.Add(new CR2WBufferWrapper(new CR2WBuffer()
                            {
                                flags = 0,                              //TODO: find out what these are
                                index = (uint)i,
                                offset = offset,
                                diskSize = zsize,
                                memSize = (uint)inbuffer.Length,
                                crc32 = crc
                            }));
                        }
                    }

                    // write cr2w headers
                    fileWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                    cr2w.WriteHeader(fileWriter);
                }
                else
                {
                    _loggerService.Error("Importing raw files into redengine files is not yet implemented.");
                    //TODO: switch and call import with existing
                    return false;
                }

                return true;
            }

            byte[] ReadBuffer(FileInfo buffer)
            {
                using var fs = new FileStream(buffer.FullName, FileMode.Open, FileAccess.Read);
                using var br = new BinaryReader(fs);
                var bext = buffer.Extension.ToLower();

                // if dds file, delete the
                if (unsaferaw && bext != ".buffer")
                {
                    switch (bext)
                    {
                        case ".dds":
                            // check if dx10
                            fs.Seek(84, SeekOrigin.Begin);
                            var fourcc = br.ReadInt32();
                            // delete dds header
                            if (fourcc == 808540228) //is dx10
                            {
                                fs.Seek(148, SeekOrigin.Begin);
                                return br.ReadBytes((int)fs.Length - 148);
                            }
                            else
                            {
                                fs.Seek(128, SeekOrigin.Begin);
                                return br.ReadBytes((int)fs.Length - 128);
                            }
                    }
                }
                else
                {
                    return Catel.IO.StreamExtensions.ToByteArray(fs);
                }

                return Array.Empty<byte>();
            }

            #endregion local methods
        }

        #endregion Methods
    }
}
