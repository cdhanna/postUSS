using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BrewedInk.PostUSS.Npm
{

   public abstract class NpmCommand
   {
      const int PROCESS_NOT_FOUND_EXIT_CODE = 127; // TODO: Check this for windows?

      private Process _process;
      private TaskCompletionSource<int> _status, _standardOutComplete;

      private bool _started, _hasExited;
      protected int _exitCode = -1;
      protected string DockerCmd => "npm";
      public Action<int> OnExit;
      public Action<string> OnStandardOut, OnStandardErr;

      public bool WriteLogToUnity { get; set; }
      public bool WriteCommandToUnity { get; set; }


      public abstract string GetCommandString();


      protected virtual void HandleOnExit()
      {
      }

      protected virtual void HandleStandardOut(string data)
      {
         if (_hasExited && data == null)
         {
            _standardOutComplete.TrySetResult(0);
         }

         if (WriteLogToUnity && data != null)
         {
            LogInfo(data);
         }
         OnStandardOut?.Invoke(data);
      }

      protected virtual void HandleStandardErr(string data)
      {
         if (WriteLogToUnity && data != null)
         {
            LogError(data);
         }
         OnStandardErr?.Invoke(data);

      }

      public virtual void Start()
      {
         if (_process != null)
         {
            throw new Exception("Process already started.");
         }

         var command = GetCommandString();
         /*do not await. It will keep it on a separate thread, which is very important. */

         Run(command);
      }

      public void Join()
      {
         _status.Task.Wait();
      }

      public void Kill()
      {
         if (_process == null || !_started || _hasExited) return;

         _process.Kill();
         try
         {
         }
         catch (InvalidOperationException ex)
         {
            Debug.LogWarning("Unable to stop process, but likely was already stopped. " + ex.Message);
         }

      }

      protected void LogInfo(string data)
      {
         Debug.Log(data);
      }

      protected void LogError(string data)
      {
         Debug.LogError(data);
      }

      protected virtual void ModifyStartInfo(ProcessStartInfo processStartInfo)
      {

      }


      async void Run(string command)
      {
         try
         {
            if (WriteCommandToUnity)
            {
               Debug.Log("============== Start Executing [" + command + "] ===============");
            }

            using (_process = new System.Diagnostics.Process())
            {

#if UNITY_EDITOR && !UNITY_EDITOR_WIN
               _process.StartInfo.FileName = "sh";
               _process.StartInfo.Arguments = $"-c '{command}'";
#else
         _process.StartInfo.FileName = "cmd.exe";
         _process.StartInfo.Arguments = $"/C {command}";  //  "/C " + command + " > " + commandoutputfile + "'"; // TODO: I haven't tested this since refactor.
#endif
               // Configure the process using the StartInfo properties.
               _process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
               _process.EnableRaisingEvents = true;
               _process.StartInfo.RedirectStandardInput = true;
               _process.StartInfo.RedirectStandardOutput = true;
               _process.StartInfo.RedirectStandardError = true;
               _process.StartInfo.CreateNoWindow = true;
               _process.StartInfo.UseShellExecute = false;
               ModifyStartInfo(_process.StartInfo);


               _status = new TaskCompletionSource<int>();
               _standardOutComplete = new TaskCompletionSource<int>();
               EventHandler eh = (s, e) =>
               {
                  // there still may pending log lines, so we need to make sure they get processed before claiming the process is complete
                  _hasExited = true;
                  _exitCode = _process.ExitCode;

                  OnExit?.Invoke(_process.ExitCode);
                  HandleOnExit();

                  _status.TrySetResult(0);
               };

               _process.Exited += eh;


               try
               {
                  _process.EnableRaisingEvents = true;

                  _process.OutputDataReceived += (sender, args) => {
                     EditorApplication.delayCall += () => {
                        try {
                           HandleStandardOut(args.Data);
                        }
                        catch (Exception ex) {
                           Debug.LogException(ex);
                        }
                     };
                  };
                  _process.ErrorDataReceived += (sender, args) => {
                     EditorApplication.delayCall += () => {
                        try {
                           HandleStandardErr(args.Data);
                        }
                        catch (Exception ex) {
                           Debug.LogException(ex);
                        }
                     };
                  };


                  _process.Start();
                  _started = true;
                  _process.BeginOutputReadLine();
                  _process.BeginErrorReadLine();

                  await _status.Task;

               }
               finally
               {
                  _process.Exited -= eh;
               }

               if (WriteCommandToUnity)
               {
                  Debug.Log("============== End ===============");
               }
            }
         }
         catch (Exception e)
         {
            Debug.LogException(e);
         }
      }

   }
}