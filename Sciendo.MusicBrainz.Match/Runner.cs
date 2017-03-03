﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3;
using Sciendo.Common.Serialization;

namespace Sciendo.MusicBrainz.Match
{
    public class Runner:IRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _path;
        private readonly string[] _extensions;
        private readonly IAnalyser _analyser;
        private readonly string _outputFilePath;
        private List<FileAnalysed> _filesAnalysed;
        private bool _fileSystemStopped=false;
        private bool _analyserStopped = false;

        public Runner(IFileSystem fileSystem, string path, string[] extensions, IAnalyser analyser, string outputFilePath)
        {
            _fileSystem = fileSystem;
            _path = path;
            _extensions = extensions;
            _analyser = analyser;
            _outputFilePath = outputFilePath;
            _filesAnalysed=new List<FileAnalysed>();
        }
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            var folders = _fileSystem.GetLeafDirectories(_path);
            foreach (var folder in folders)
            {
                if (_fileSystem.StopActivity || _analyser.StopActivity)
                {
                    Serializer.SerializeToFile(_filesAnalysed,_outputFilePath);
                    return;
                }
                _filesAnalysed.AddRange(RunDirectory(folder));
            }
            Serializer.SerializeToFile(_filesAnalysed,_outputFilePath);
        }

        private IEnumerable<FileAnalysed> RunDirectory(string path)
        {
            var previousArtist = string.Empty;
            var files = _fileSystem.GetFiles(path, _extensions);
            foreach (var file in files)
            {
                if (_fileSystem.StopActivity || _analyser.StopActivity)
                {
                    Serializer.SerializeToFile(_filesAnalysed,_outputFilePath);
                    break;
                }
                var fileAnalysed = _analyser.AnalyseFile(new Mp3File(file), file);
                if (!string.IsNullOrEmpty(previousArtist) && fileAnalysed.Id3TagComplete &&
                    previousArtist != fileAnalysed.Id3Tag.Artists.TextValue)
                {
                    fileAnalysed.PossiblePartOfACollection = true;
                }
                yield return fileAnalysed;
            }
        }

        public void Stop()
        {
            _fileSystem.StopActivity = true;
            _analyser.StopActivity = true;
            Serializer.SerializeToFile(_filesAnalysed, _outputFilePath);
        }

        public List<FileAnalysed> FilesAnalysed { get {return _filesAnalysed;} }
    }
}