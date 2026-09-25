$c = Get-Content Views\Employees\Index.cshtml -Raw -Encoding UTF8
$c = $c -replace '(?s)<a href="@Url\.Action\("Delete".*?</a>', '<button type="button" class="action-icon delete-action border-0 bg-transparent" title="Delete" onclick="confirmDelete(@item.EmployeeId, ''@item.FullName.Replace("''", "\''")'')"> <i class="fa-solid fa-trash text-danger"></i> </button>'
Set-Content Views\Employees\Index.cshtml $c -Encoding UTF8
