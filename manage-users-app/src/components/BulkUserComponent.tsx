import React, { useState } from "react";
import { createBulkUsers } from "../services/userService";

interface Props {
  onBulkCreated: () => void;
}

const BulkUserComponent: React.FC<Props> = ({ onBulkCreated }) => {
  const [loading, setLoading] = useState(false);
  const [apiError, setApiError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  const handleBulk = async () => {
    setApiError(null);
    setSuccessMsg(null);
    try {
      setLoading(true);
      const { success, message } = await createBulkUsers();
      if (success) {
        setSuccessMsg(message);
        onBulkCreated();
      } else {
        setApiError(message);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mb-4">
      {apiError && <div className="alert alert-danger alert-dismissible">{apiError}</div>}
      {successMsg && <div className="alert alert-success alert-dismissible">{successMsg}</div>}
      <button className="btn btn-success" onClick={handleBulk} disabled={loading}>
        {loading ? "Creating 10,000..." : "Create 10,000 Users"}
      </button>
    </div>
  );
};

export default BulkUserComponent;
