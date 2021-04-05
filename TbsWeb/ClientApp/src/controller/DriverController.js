import axios from 'axios';

export async function login (index) {
    if (index < 0) return null;
    try {
        await axios.post(`/driver/login/${index}`);
    } catch (e) {
        console.log(e);
    }
    return null;
}

export async function logout (index) {
    if (index < 0) return null;
    try {
        await axios.post(`/driver/logout/${index}`);
    } catch (e) {
        console.log(e);
    }
    return null;
}

export async function getStatus (index) {
    if (index < 0) return null;
    try {
        const { data } = await axios.get(`/driver/status/${index}`);
        return data;
    } catch (e) {
        console.log(e);
    }
    return null;
}
