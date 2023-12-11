using Firebase.Storage;
using Google.Cloud.Firestore;
using WebAppCondominio.FirebaseAuth;

namespace TaskManagerApp.Models
{
    public class Tarea
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? DueDate { get; set; }
        public string? IsCompleted { get; set; }
        public string? Priority { get; set; }
    }

    public class TasksHandler
    {
        public async Task<List<Tarea>> GetTasksCollection()
        {
            List<Tarea> taskList = new List<Tarea>();
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Tasks");
            QuerySnapshot querySnaphot = await query.GetSnapshotAsync();

            foreach (var item in querySnaphot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                taskList.Add(new Tarea
                {
                    Id = item.Id,
                    Title = data["Title"].ToString(),
                    Description = data["Description"].ToString(),
                    DueDate = data["DueDate"].ToString(),
                    IsCompleted = data["IsCompleted"].ToString(),
                    Priority = data["Priority"].ToString(),
                });
            }

            return taskList;
        }

        public async Task<bool> Create(string title, string description, string duedate, string iscompleted, string priority)
        {
            try
            {
                DocumentReference addedDocRef =
                    await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                        .Collection("Tasks").AddAsync(new Dictionary<string, object>
                            {
                                { "Title", title },
                                { "Description", description },
                                { "DueDate", duedate },
                                { "IsCompleted",  iscompleted },
                                { "Priority", priority }
                            });

                return true;
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Edit(string id, string title, string description, string duedate, string iscompleted, string priority)
        {
            try
            {
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Tasks").Document(id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    { "Title", title },
                    { "Description", description },
                    { "DueDate", duedate },
                    { "IsCompleted",  iscompleted },
                    { "Priority", priority }
                };
                WriteResult result = await docRef.UpdateAsync(dataToUpdate);

                return true;
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }

        public async Task<Tarea> GetTaskById(string id)
        {
            try
            {
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Tasks").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    Dictionary<string, object> data = snapshot.ToDictionary();

                    return new Tarea
                    {
                        Id = snapshot.Id,
                        Title = data["Title"].ToString(),
                        Description = data["Description"].ToString(),
                        DueDate = data["DueDate"].ToString(),
                        IsCompleted = data["IsCompleted"].ToString(),
                        Priority = data["Priority"].ToString(),
                    };
                }

                return null; // Devolver null si la tarea no existe
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }
    }
}