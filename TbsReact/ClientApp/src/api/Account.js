import axios from 'axios'

const getAccounts = async () => {
    try {
        const {data} = await axios.get("/accounts");
        return data;
    }
    catch (e) {
        console.log(e);
    }
}

const getAccount = async (index) => {
    try {
        const {data} = await axios.get(`/accounts/${index}`);
        return data;
    }
    catch (e) {
        console.log(e);
    }
}
const addAccount = async (data) => {
    try {
        await axios.post('/accounts', data);
    } catch (e) {
        console.log(e);
    }
}

const deleteAccount = async (index) => {
    try {
        await axios.delete(`/accounts/${index}`);
    } catch (e) {
        console.log(e);
    }
}