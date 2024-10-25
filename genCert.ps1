$thumbprint = "CEAC2449D10427A4AC7A6434CB5C6BB9C4DAF6FD"
$cert = Get-ChildItem -Path cert:\LocalMachine\My\$thumbprint
Export-Certificate -Cert $cert -FilePath C:\temp\MyCertificate.cer
