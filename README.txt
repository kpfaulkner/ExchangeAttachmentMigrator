Exchange Attachment Migrator

Aim:

Polls an Exchange server and looks for email attachments. Uploads attachments to Azure (for example) and then modifies the link in Exchange to point to "Exchange Attachment Migrator" service. 
The original attachment from Exchange is deleted. 
When the link within Exchange is used EAM checks the user has permission to see the attachment then redirects to the attachment blob.

At least, this is the theory....
