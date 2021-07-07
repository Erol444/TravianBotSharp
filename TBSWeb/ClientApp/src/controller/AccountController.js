import axios from 'axios';

export async function getListAccounts () {
    try {
        const { data } = await axios.get('/accounts');

        data.forEach(acc => {
            acc.serverUrl = acc.serverUrl.replace(/(^\w+:|^)\/\//, '');
        });
        return data;
    } catch (e) {
        console.log(e);
    }
    return null;
}

export async function getAccount (index) {
    try {
        const { data } = await axios.get(`/accounts/${index}`);
        data.serverUrl = data.serverUrl.replace(/(^\w+:|^)\/\//, '');
        return data;
    } catch (e) {
        console.log(e);
    }
    return null;
}

export async function addAccount (data) {
    try {
        await axios.post('/accounts', data);
    } catch (e) {
        console.log(e);
    }
}

export async function editAccount (index, data) {
    try {
        await axios.put(`/accounts/${index}`, data);
    } catch (e) {
        console.log(e);
    }
}

export async function deleteAccount (index) {
    try {
        await axios.delete(`/accounts/${index}`);
    } catch (e) {
        console.log(e);
    }
}

export async function getTasks (index) {
    try {
        console.log(index);
        const { data } = await axios.get(`/debug/tasks/${index}`);
        console.log(data);
        return data;
    } catch (e) {
        console.log(e);
    }
    return null;
}
const current = {
    account: -1,
    village: -1,
};

Object.seal(current);
export { current };
