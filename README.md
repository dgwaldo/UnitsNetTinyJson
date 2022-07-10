# UnitsNet Tiny JSON
A size conscious serializer for UnitsNet.

## About
This project provides an example of an alternate serializer for UnitsNet. This project uses NewtonSoft.Json for serialization.

<code>
// Single Value
{  "weight": "90.0|kg" }
</code>
<br/>
<code>
// Array
{  "weights": ["90.0|kg","90.1|kg","90.2|kg"] }
</code>

## Getting Started

Run the project TinyJson.WebAPI. Use the Swagger UI to make a request to the get endpoint. Post into the SenUnits endpoint. 

  * Program.cs 
      * contains the wire-up for JSON serializers in the AddNewtonsoftJson extension method.
      * has wire-up for Swagger UI, note the MapType<> call. This sets Swagger show the simple unit "val|unit" instead of the full units model.
  * Unit Tests
    * covers basic cases
    * show the basic way the JSON converter can be used
  
## Built With
  * [UnitsNet](https://github.com/angularsen/UnitsNet)
  * [Newtonsoft JSON](https://www.newtonsoft.com/json/help/html/Introduction.htm)
  * [.Net Core](https://dotnet.microsoft.com/en-us/download)
  
## License 

MIT No Attribution

Copyright <YEAR> <COPYRIGHT HOLDER>

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
