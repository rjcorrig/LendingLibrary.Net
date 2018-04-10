#!/bin/bash
mono --debug --profile=log:coverage,covfilter=+LendingLibrary,covfilter=-LendingLibrary.Tests,output=coverage/coverage.mlpd bin/Debug/LendingLibrary.Tests.exe --noresult
mprof-report --reports=coverage --coverage-out=coverage/coverage.xml coverage/coverage.mlpd
mono ./lib/ReportGenerator.3.1.0/tools/ReportGenerator.exe -reports:coverage/coverage.xml -targetdir:coverage/report
open ./coverage/report/index.htm 
