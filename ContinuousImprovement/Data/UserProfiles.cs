using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousImprovement.Data
{
    [FirestoreData]
    public class UserProfiles
    {
        [FirestoreProperty]
        public String Id { get; set; }
        [FirestoreProperty]
        public String department { get; set; }
        [FirestoreProperty]
        public String employeeId { get; set; }
        [FirestoreProperty]
        public String fullName { get; set; }
    }
}
