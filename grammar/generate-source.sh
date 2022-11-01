#!/bin/sh
echo '--------------------------------------------'
echo ' source generation C# - rsql                '
echo '--------------------------------------------'
java -jar antlr4-4.9.3-complete.jar RSqlQuery.g -o ../src/RSql4net/Models/Queries -Dlanguage=CSharp -listener -encoding UTF-8 -visitor -package RSql4Net.Models.Queries


