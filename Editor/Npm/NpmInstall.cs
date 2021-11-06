using System.Diagnostics;
using System.IO;

namespace BrewedInk.PostUSS.Npm
{
   public class NpmInstall : NpmCommand
   {
      private readonly PostUssConfiguration _configuration;

      public NpmInstall(PostUssConfiguration configuration)
      {
         _configuration = configuration;
      }

      protected override void ModifyStartInfo(ProcessStartInfo processStartInfo)
      {
         processStartInfo.WorkingDirectory = Path.GetDirectoryName(_configuration.compilePath);
      }

      public override string GetCommandString()
      {
         return "npm install --loglevel=error";
      }
   }
}