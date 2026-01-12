import axios from "axios";
import { UserDto } from "../types/UserDto";

const API_BASE_URL =
  process.env.REACT_APP_API_BASE_URL || "https://localhost:7247/api/users";

interface ApiResult<T> {
  isSuccess: boolean;
  statusCode: number;
  data?: T;
  errors: string[];
}


export const fetchUsers = async (): Promise<{ data: UserDto[]; errors: string[] }> => {
  try {
    const response = await axios.get<ApiResult<UserDto[]>>(`${API_BASE_URL}/fetch-users`);
    if (response.data.isSuccess) {
      return { data: response.data.data || [], errors: [] };
    } else {
      return { data: [], errors: response.data.errors };
    }
  } catch (err: any) {
    return { data: [], errors: [err.message] };
  }
};

export const createUser = async (user: UserDto): Promise<{ success: boolean; message: string }> => {
  try {
    const response = await axios.post<ApiResult<string>>(`${API_BASE_URL}/create-user`, user);
    if (response.data.isSuccess) {
      return { success: true, message: response.data.data || "User created successfully" };
    } else {
      return { success: false, message: response.data.errors.join(", ") };
    }
  } catch (err: any) {
    return { success: false, message: err.message };
  }
};


export const createBulkUsers = async (): Promise<{ success: boolean; message: string }> => {
  try {
    const response = await axios.post<ApiResult<string>>(`${API_BASE_URL}/create-bulk-users`);
    if (response.data.isSuccess) {
      return { success: true, message: response.data.data || "Bulk users created successfully" };
    } else {
      return { success: false, message: response.data.errors.join(", ") };
    }
  } catch (err: any) {
    return { success: false, message: err.message };
  }
};
