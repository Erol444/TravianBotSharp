import axios from "axios";

const getChromeSetting = async (index) => {
	try {
		const { data } = await axios.get(`/settings_chrome/${index}`);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const setChromeSetting = async (index, data) => {
	try {
		const patchData = {
			click: {
				min: data.click_min,
				max: data.click_max,
			},
			workTime: {
				min: data.work_min,
				max: data.work_max,
			},
			sleepTime: {
				min: data.sleep_min,
				max: data.sleep_max,
			},
		};
		await axios.patch(`/settings_chrome/${index}/`, patchData);
	} catch (e) {
		console.log(e);
	}
};
export { getChromeSetting, setChromeSetting };
