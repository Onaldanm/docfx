﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class FileAndType
        : IEquatable<FileAndType>
    {
        public FileAndType(string baseDir, string file, DocumentType type, string sourceDir = null, string destinationDir = null)
        {
            if (baseDir == null)
            {
                throw new ArgumentNullException(nameof(baseDir));
            }
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (!Path.IsPathRooted(baseDir))
            {
                throw new ArgumentException("Base directory must be rooted.", nameof(baseDir));
            }
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentException("File cannot be empty or whitespace.", nameof(file));
            }
            if (Path.IsPathRooted(file))
            {
                throw new ArgumentException("File cannot be rooted.", nameof(file));
            }
            if (!string.IsNullOrEmpty(sourceDir) && Path.IsPathRooted(sourceDir))
            {
                throw new ArgumentException("File cannot be rooted.", nameof(sourceDir));
            }
            if (!string.IsNullOrEmpty(destinationDir) && Path.IsPathRooted(destinationDir))
            {
                throw new ArgumentException("File cannot be rooted.", nameof(destinationDir));
            }

            BaseDir = baseDir.Replace('\\', '/');
            File = file.Replace('\\', '/');
            Type = type;
            FullPath = Path.Combine(BaseDir, File).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            SourceDir = sourceDir?.Replace('\\', '/') ?? string.Empty;
            DestinationDir = destinationDir?.Replace('\\', '/') ?? string.Empty;
            StringComparer = GetStringComparer();
        }

        [Obsolete]
        public FileAndType(string baseDir, string file, DocumentType type, Func<string, string> pathRewriter)
            : this(baseDir, file, type, null, null)
        {
            PathRewriter = pathRewriter;
        }

        public StringComparer StringComparer { get; }

        public string BaseDir { get; }

        public string File { get; }

        public string FullPath { get; }

        public DocumentType Type { get; }

        public string SourceDir { get; set; }

        public string DestinationDir { get; set; }

        [Obsolete]
        public Func<string, string> PathRewriter { get; }

        public FileAndType ChangeBaseDir(string baseDir)
        {
            if (PathRewriter != null)
            {
                return new FileAndType(baseDir, File, Type, PathRewriter);
            }
            return new FileAndType(baseDir, File, Type, SourceDir, DestinationDir);
        }

        public FileAndType ChangeFile(string file)
        {
            if (PathRewriter != null)
            {
                return new FileAndType(BaseDir, file, Type, PathRewriter);
            }
            return new FileAndType(BaseDir, file, Type, SourceDir, DestinationDir);
        }

        public FileAndType ChangeType(DocumentType type)
        {
            if (PathRewriter != null)
            {
                return new FileAndType(BaseDir, File, type, PathRewriter);
            }
            return new FileAndType(BaseDir, File, type, SourceDir, DestinationDir);
        }

        public bool Equals(FileAndType other)
        {
            if (other == null)
            {
                return false;
            }
            return StringComparer.Equals(File, other.File) &&
                Type == other.Type &&
                StringComparer.Equals(BaseDir, other.BaseDir) &&
                StringComparer.Equals(SourceDir, other.SourceDir) &&
                StringComparer.Equals(DestinationDir, other.DestinationDir);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileAndType);
        }

        public override int GetHashCode()
        {
            return StringComparer.GetHashCode(File) + (int)Type ^ StringComparer.GetHashCode(BaseDir);
        }

        public static bool operator ==(FileAndType left, FileAndType right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null))
            {
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(FileAndType left, FileAndType right)
        {
            return !(left == right);
        }

        private static StringComparer GetStringComparer()
        {
            if (Environment.OSVersion.Platform < PlatformID.Unix)
            {
                return StringComparer.OrdinalIgnoreCase;
            }
            else
            {
                return StringComparer.Ordinal;
            }
        }
    }
}
