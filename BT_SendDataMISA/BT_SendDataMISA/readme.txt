Create service windows
1. Publish Project
	Configuration: Release | Any CPU
	Deployment Mode: Framework Dependent
	Target Runtime: Portable
2. Open Windows PowerShell
	sc.exe create [name_service] binpath=C:\Temp\WorkerService\Service.exe start=auto
	sc.exe delete [name_service]