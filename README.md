# DotNetCoreSpecflowBase

to fix the error
The process cannot access the file 'C:\\bin\\Debug\\netcoreapp3.1\\chromedriver.exe' because it is being used by another process

Add additional logic to your hooks
``Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
                foreach (var chromeDriverProcess in chromeDriverProcesses)
                {
                    chromeDriverProcess.Kill();
                }``
                
