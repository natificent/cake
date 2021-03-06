﻿using System;
using System.Linq;

namespace Cake.Core.IO.NuGet
{
    /// <summary>
    /// Contains NuGet path resolver functionality
    /// </summary>
    public sealed class NuGetToolResolver : INuGetToolResolver
    {        
        private readonly IFileSystem _fileSystem;
        private readonly IGlobber _globber;
        private readonly ICakeEnvironment _environment;
        private IFile _nugetExeFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetToolResolver" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="globber">The globber.</param>
        /// <param name="environment">The environment.</param>
        public NuGetToolResolver(IFileSystem fileSystem, IGlobber globber, ICakeEnvironment environment)
        {
            _fileSystem = fileSystem;
            _globber = globber;
            _environment = environment;
        }

        /// <summary>
        /// Gets the tool name.
        /// </summary>
        /// <value>The tool name.</value>
        public string Name
        {
            get { return "NuGet"; }
        }

        /// <summary>
        /// Resolves the tool path.
        /// </summary>
        /// <returns>
        /// The tool path.
        /// </returns>
        /// <exception cref="CakeException">No nuget.exe found by resolver.</exception>
        public FilePath ResolveToolPath()
        {
            // Check if path allready resolved
            if (_nugetExeFile != null && _nugetExeFile.Exists)
            {
                return _nugetExeFile.Path;
            }

            // Check if path set to environment variable
            var environmentExe = _environment.GetEnvironmentVariable("NUGET_EXE");
            if (!string.IsNullOrWhiteSpace(environmentExe))
            {
                var envFile = _fileSystem.GetFile(environmentExe);
                if (envFile.Exists)
                {
                    return (_nugetExeFile = envFile).Path;
                }
            }

            // Check if tool exists in tool folder
            const string expression = "./tools/**/NuGet.exe";
            var toolsExe = _globber.GetFiles(expression).FirstOrDefault();
            if (toolsExe != null)
            {
                var toolsFile = _fileSystem.GetFile(toolsExe);
                if (toolsFile.Exists)
                {
                    return (_nugetExeFile = toolsFile).Path;
                }
            }

            // Last resort try path
            var envPath = _environment.GetEnvironmentVariable("path");
            if (!string.IsNullOrWhiteSpace(envPath))
            {
                var pathFile = envPath
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => _fileSystem.GetDirectory(path))
                    .Where(path => path.Exists)
                    .Select(path => path.Path.CombineWithFilePath("nuget.exe"))
                    .Select(_fileSystem.GetFile)
                    .FirstOrDefault(file => file.Exists);

                if (pathFile != null)
                {
                    return (_nugetExeFile = pathFile).Path;
                }
            }

            throw new CakeException("No nuget.exe found by resolver.");
        }
    }
}
