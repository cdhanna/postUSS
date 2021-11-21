using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BrewedInk.PostUSS.Npm
{
   public class NpmRun : NpmCommand
   {
      private readonly PostUssConfiguration _configuration;
      private readonly string _command;
      private readonly Dictionary<string, string> _env;

      public NpmRun(PostUssConfiguration configuration, string command, Dictionary<string, string> env=null)
      {
         _configuration = configuration;
         _command = command;
         _env = env ?? new Dictionary<string, string>();
      }

      protected override void ModifyStartInfo(ProcessStartInfo processStartInfo)
      {
         processStartInfo.WorkingDirectory = Path.GetDirectoryName(_configuration.compilePath);
         foreach (var kvp in _env)
         {
            processStartInfo.Environment.Add(kvp.Key, kvp.Value);
         }
      }


      public override string GetCommandString()
      {
         return $"npm run {_command}";
      }
   }
}