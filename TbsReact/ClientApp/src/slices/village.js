import { createSlice } from "@reduxjs/toolkit";

const initialState = {
	info: {
		id: -1,
		name: 1,
		coords: {
			x: 1,
			y: 1,
		},
	},
};

export const villageSlice = createSlice({
	name: "village",
	initialState,
	reducers: {
		setVillage: (state, action) => {
			state.info = action.payload;
		},

		resetVillage: (state, action) => {
			return initialState;
		},
	},
});

export const { setVillage, resetVillage } = villageSlice.actions;
export default villageSlice.reducer;
