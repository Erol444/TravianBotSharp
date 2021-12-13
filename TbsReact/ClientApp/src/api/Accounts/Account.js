import axios from "axios";

const getAccounts = async () => {
  try {
    const { data } = await axios.get("/accounts");
    return data;
  } catch (e) {
    console.log(e);
  }
};

const getAccount = async (index) => {
  try {
    const { data } = await axios.get(`/accounts/${index}`);
    return data;
  } catch (e) {
    console.log(e);
  }
};
const addAccount = async (acc) => {
  try {
    const { data } = await axios.post("/accounts", acc);
    return data;
  } catch (e) {
    console.log(e);
  }
};
const editAccount = async (index, data) => {
  try {
    await axios.patch(`/accounts/${index}`, data);
  } catch (e) {
    console.log(e);
  }
};

const deleteAccount = async (index) => {
  try {
    await axios.delete(`/accounts/${index}`);
  } catch (e) {
    console.log(e);
  }
};

export { getAccounts, getAccount, addAccount, editAccount, deleteAccount };
