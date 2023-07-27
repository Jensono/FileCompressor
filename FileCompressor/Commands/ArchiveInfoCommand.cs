﻿//-----------------------------------------------------------------------
// <copyright file="ArchiveInfoCommand.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This is the class for the Info command. When executed it displays meta information about an archive file.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;
    using System.IO;
    public class ArchiveInfoCommand : IArchiveCommand
    {
        
        private string archiveSource;

        public ArchiveInfoCommand(string source)
        {
            this.ArchiveSource = source;
        }

        public string ArchiveSource
        {
            get
            {
                return this.archiveSource;
            }

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(this.ArchiveSource)} cannot be null!");
                }

                this.archiveSource = value;
            }
        }

        public bool Execute()
        {
            if (File.Exists(this.ArchiveSource))
            {
                try
                {
                    this.ReadAndConvertArchiveHeader();
                }
                catch (ArchiveErrorCodeException e)
                {
                    throw e;
                }
            }
            else
            { // TODO CONVERT TO ERROR CODE
                throw new ArchiveErrorCodeException($"The file at {this.ArchiveSource} was not found. ");
            }

            return true;
        }

        // for reading the file header we dont need a specific compressionAlogrithm as all headers are written the same, regardeless of compression
        private void ReadAndConvertArchiveHeader()
        {
            try
            {
                ArchiveFileReader archiveReader = new ArchiveFileReader(this.ArchiveSource);
                archiveReader.ReadArchiveHeaderAndPrintToConsole();
            }
            catch (ArchiveErrorCodeException e)
            {
                throw e;
            }
        }
    }
}