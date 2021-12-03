import axios from 'axios';

const login = async (index) => {
    try {
        await axios.post(`/accounts/login/${index}`);
        return true;
    } catch (e) {
        console.log(e);
        return false;
    }
}

const logout = async (index) => {
    try {
        await axios.post(`/accounts/logout/${index}`);
        return true;
    } catch (e) {
        console.log(e);
        return true;
    }
}

const loginAll = async () => {
    try {
        await axios.post(`/accounts/login`);
        return true;
    } catch (e) {
        console.log(e);
        return false;
    }
}

const logoutAll = async () => {
    try {
        await axios.post(`/accounts/logout`);
        return true;
    } catch (e) {
        console.log(e);
        return false;
    }
}

const getStatus = async (index) => {
    try {
        const { data } = await axios.get(`/accounts/status/${index}`);
        return data;
    } catch (e) {
        console.log(e);
        return false;
    }
}

export { login, logout, loginAll, logoutAll, getStatus }