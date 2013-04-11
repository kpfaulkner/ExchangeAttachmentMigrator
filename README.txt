Exchange Attachment Migrator

Aim:

Polls an Exchange server and looks for email attachments. Uploads attachments to Azure (for example) and then modifies the link in Exchange to point to itself.
When the link is used, it checks if the user has permission for the original attachment (no idea how to do that yet) then redirects to the Azure blob.

Original attachment in Exchange is removed, saving space.

All theory for now...