import axios from 'axios'

const getLogData = async (index) => {
    try {
        const {data} = await axios.get(`/accounts/${index}/log`);
        return data;
    }
    catch (e) {
        console.log(e);
    }
}

export {getLogData}