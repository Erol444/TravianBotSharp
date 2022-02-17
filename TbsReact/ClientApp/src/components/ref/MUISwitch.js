import React from "react";
import { FormControlLabel, Switch } from "@mui/material";
import { Controller } from "react-hook-form";

import PropTypes from "prop-types";

// src for who wonder what is this
// https://react-hook-form.com/advanced-usage#ControlledmixedwithUncontrolledComponents

const VillageSelect = ({ name, label, control }) => {
	return (
		<Controller
			render={({ field: { onChange, onBlur, ref, value } }) => (
				<FormControlLabel
					control={
						<Switch
							onBlur={onBlur}
							inputRef={ref}
							onChange={onChange}
							checked={value || false}
						/>
					}
					label={label}
				/>
			)}
			name={name}
			control={control}
		/>
	);
};

VillageSelect.propTypes = {
	control: PropTypes.object.isRequired,
	label: PropTypes.string.isRequired,
	name: PropTypes.string.isRequired,
};

export default VillageSelect;
