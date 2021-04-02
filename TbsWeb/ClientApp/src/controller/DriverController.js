import axios from 'axios';

export async function login (index) {
    try {
        await axios.post(`/driver/login/${index}`);
    } catch (e) {
        console.log(e);
    }
    return null;
}

export async function logout (index) {
    try {
        await axios.post(`/driver/logout/${index}`);
    } catch (e) {
        console.log(e);
    }
    return null;
}
