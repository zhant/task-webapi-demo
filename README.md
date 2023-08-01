# CNVS Demo Web API project.

The initial looze requirements are:

## Tasks management WEB-API

1. Create a simple web api to create and read Users and Tasks.
2. Users are defined by unique Name.
3. Tasks are defined by their Description and a State (Waiting, InProgress, Completed).
4. When a Task is created, an User should be assigned to it automatically.
5. Users can have multiple tasks, Task can only have one user.
6. Every 2 minutes all tasks should be reassigned to another random user (it can't be the user which is already assigned to the task).
7. When no users are available the Task will stay without assigned user.
8. All task have to be transferred for exactly 3 times, after that, they should be considered completed and stay unassigned.
9. No user interface is requirement for the assignment.

We purposely left requirements loose.
