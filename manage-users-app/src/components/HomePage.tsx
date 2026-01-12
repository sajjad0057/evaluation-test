import React, { useEffect, useState } from "react";
import { fetchUsers } from "../services/userService";
import { UserDto } from "../types/UserDto";
import CreateUser from "./CreateUser";
import BulkUserComponent from "./BulkUserComponent";

const HomePage: React.FC = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadUsers = async () => {
    setLoading(true);
    setError(null);
    const { data, errors } = await fetchUsers();
    if (errors.length > 0) {
      setError(errors.join(", "));
      setUsers([]);
    } else {
      setUsers(data);
    }
    setLoading(false);
  };

  useEffect(() => {
    loadUsers();
  }, []);

  return (
    <div className="container py-5">
      <h1 className="mb-4">Manage Users</h1>

      <CreateUser onUserCreated={loadUsers} />
      <BulkUserComponent onBulkCreated={loadUsers} />

      {error && (
        <div className="alert alert-warning mt-3 alert-dismissible" role="alert">
          {error}
        </div>
      )}

      <h2 className="mt-5 mb-3">User List</h2>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="table-responsive">
          <table className="table table-bordered table-hover">
            <thead className="table-light">
              <tr>
                <th>Name</th>
                <th>Age</th>
                <th>Email</th>
                <th>TimeStamp</th>
              </tr>
            </thead>
            <tbody>
              {users.map(u => (
                <tr key={u.id}>
                  <td>{u.name}</td>
                  <td>{u.age}</td>
                  <td>{u.email}</td>
                  <td>{u.timeStamp}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default HomePage;
