#!/bin/sh

# NamingFormatter - String format library with key-valued replacer.
# Copyright (c) 2019 Kouji Matsui (@kekyo2)
# 
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# 
# http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

echo ""
echo "==========================================================="
echo "Build NamingFormatter"
echo ""

dotnet build -p:Configuration=Release -p:Platform="Any CPU" -p:RestoreNoCache=True NamingFormatter.sln
dotnet pack -p:Configuration=Release -p:Platform="Any CPU" -o artifacts NamingFormatter.sln
