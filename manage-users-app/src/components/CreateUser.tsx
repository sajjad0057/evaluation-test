import React, { useState } from "react";
import { createUser } from "../services/userService";
import { UserDto } from "../types/UserDto";

interface Props {
  onUserCreated: () => void;
}

const CreateUser: React.FC<Props> = ({ onUserCreated }) => {
  const [name, setName] = useState("");
  const [age, setAge] = useState(0);
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [nameError, setNameError] = useState("");
  const [ageError, setAgeError] = useState("");
  const [apiError, setApiError] = useState<string | null>(null);

  const handleSubmit = async () => {
    let valid = true;

    if (name.trim() === "") {
      setNameError("Name cannot be empty");
      valid = false;
    } else setNameError("");

    if (age < 1) {
      setAgeError("Age must be at least 1");
      valid = false;
    } else setAgeError("");

    if (!valid) return;

    setApiError(null);
    try {
      setLoading(true);
      const { success, message } = await createUser({ name, age, email } as UserDto);
      if (success) {
        setName("");
        setAge(0);
        setEmail("");
        onUserCreated();
      } else {
        setApiError(message);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card mb-4">
      <div className="card-body">
        <h5 className="card-title">Create User</h5>
        {apiError && <div className="alert alert-danger alert-dismissible">{apiError}</div>}

        <div className="row g-2 align-items-center">
          <div className="col">
            <input
              type="text"
              className={`form-control ${nameError ? "is-invalid" : ""}`}
              placeholder="Name"
              value={name}
              onChange={e => setName(e.target.value)}
            />
            {nameError && <div className="invalid-feedback">{nameError}</div>}
          </div>

          <div className="col-2">
            <input
              type="number"
              className={`form-control ${ageError ? "is-invalid" : ""}`}
              placeholder="Age"
              value={age}
              onChange={e => setAge(Number(e.target.value))}
            />
            {ageError && <div className="invalid-feedback">{ageError}</div>}
          </div>

          <div className="col">
            <input
              type="email"
              className="form-control"
              placeholder="Email"
              value={email}
              onChange={e => setEmail(e.target.value)}
            />
          </div>

          <div className="col-auto">
            <button className="btn btn-primary" onClick={handleSubmit} disabled={loading}>
              {loading ? "Creating..." : "Create"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CreateUser;
