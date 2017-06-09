echo on

set Path=ProtoGen\protogen.exe

 
%Path%  -i:Request.proto    --java_out=.\OpenAPIModel\Request.cs

%Path%  -i:Response.proto    --java_out=.\OpenAPIModel\Response.cs

%Path%  -i:UserInfo.proto    --java_out=.\OpenAPIModel\UserInfo.cs

%Path%  -i:LoginReq.proto    --java_out=.\OpenAPIModel\LoginReq.cs

%Path%  -i:LoginResp.proto    --java_out=.\OpenAPIModel\LoginResp.cs

pause