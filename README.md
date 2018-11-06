# DiffManager
DiffManager allows for comparison between two binary data, which are passed to the Web API as JSON base64 encoded data.

The data is persisted in a in-memory data store and available for comparison.


## PREREQUISITES
In order to be able to explore the solution and run the application, the following prerequisites are needed:
*  .NET Core 2.1 SDK (2.1.1) has to be installed on the machine. It can be downloaded from [here](https://www.microsoft.com/net/download/dotnet-core/2.1)
*  To use .NET Core 2.1 with Visual Studio, you'll need Visual Studio 2017 15.7 or newer. Make sure you've got the latest version of Visual Studio.


## USAGE
The Web API provides REST(y) endpoints from which the difference data can be added, updated and compared.__
The ID is expressed in a form of a GUID. This guarantees that the logic for persisting the data is independent from the data store that is actually used throughout the application.__

**Insert or Update**:
*<host>/v1/diff/<ID>/left* and *<host>/v1/diff/<ID>/right*__
These endpoints are used to add or update right and left data independently via a HTTP POST request.__
The body of the request is comprised of an object which contains a mandatory property 'value'. That holds the base64 encoded data.

Example:__
URL: 
*https://localhost:44309/v1/diff/df924786-292c-4dfc-80b4-a9fa6b2fba07/left*

METHOD:
HTTP POST

BODY:
```
{
	value: 'Q2lhbzAwbW9uZG8w'
}
```

RETURN:
**200 OK** status code if the request was successful. No data will be returned.__
If the resource doesn't exist, it will be created and the value will be assign to the right or left field.__
If the resource already exists, the data will be updated with the new value.__
**400 Bad Request** if the body doesn't contain the required fields.


**Create**:
*<host>/v1/diff*
This endpoint is used to create a new resource containing both left and right data via a HTTP POST request.
The body of the request is comprised of an object which contains two mandatory properties 'left' and 'right'. They hold the base64 encoded data.

Example:
URL: 
*https://localhost:44309/v1/diff*

METHOD:
HTTP POST

BODY:
```
{
	left: 'Y2lhbw==',
	right: 'bW9uZG8='
}
```

RETURN:
**200 OK** status code if the request was successful. The newly created resource will be returned.
**400 Bad Request** if the body doesn't contain the required fields.


**Full Update**:
*<host>/v1/diff/<ID>*
This endpoint is used to fully update an existing resource with both left and right data via a HTTP PUT request.
The body of the request is comprised of an object which contains two non mandatory properties 'left' and 'right'. They hold the base64 encoded data.

Example:
URL: 
*https://localhost:44309/v1/diff/df924786-292c-4dfc-80b4-a9fa6b2fba07*

METHOD:
HTTP PUT

BODY:
```
{
	left: 'Y2lhbw==',
	right: 'bW9uZG8='
}
```

RETURN:
**204 No Content** status code if the request was successful.


**Comparison**:
*<host>/v1/diff/<ID>*
This endpoint is used to compare the left and right data of a resource via a HTTP GET request.

Example:
URL: 
*https://localhost:44309/v1/diff/df924786-292c-4dfc-80b4-a9fa6b2fba07*

METHOD:
HTTP GET

RETURN:
**200 OK** status code if the request was successful.
The data that is returned is an object containing the property 'result'. 
It can contain different values depending on the outcome of the comparison:
If left and right data
*  are equal:
  ```
  {
      "result": "No differences"
  }
  ```
*  have different length:
  ```
  {
      "result": "The length is not the same"
  }
  ```
*  are different:
  An array of differences is returned, each one containing its offset and length.
  ```
  {
      "result": [
          {
              "offset": 4,
              "length": 2
          },
          {
              "offset": 11,
              "length": 1
          }
      ]
  }
  ```
**404 Not Found** if the resource does not exist.


## AREAS OF IMPROVEMENT:
*  BSON support for Web APIs to improve performance.
*  partial updates capability using HTTP PATCH request. This would make <host>/v1/diff/<ID>/left and <host>/v1/diff/<ID>/right endpoints deprecated.
*  delete capability using HTTP DELETE request.
*  to be more REST compliant, the comparison should be done from an endpoint that looks like <host>/v1/diff/<ID>/comparison, which should be implemented in a separate controller.
*  Add Swagger documentation.
