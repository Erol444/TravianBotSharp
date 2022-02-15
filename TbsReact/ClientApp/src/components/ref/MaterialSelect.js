import React from "react";
import { FormControl, InputLabel, Select } from "@mui/material";
import { Controller } from "react-hook-form";

import PropTypes from "prop-types";

// src for who wonder what is this
// https://dev.to/raduan/4-ways-to-use-material-ui-select-with-react-hook-form-41b2
// because we use react-hook-form v7, "as" become "render"
// you can check their documment for more info
const MaterialSelect = ({
	name,
	label,
	control,
	defaultValue,
	children,
	...props
}) => {
	const labelId = `${name}-label`;

	return (
		<FormControl {...props}>
			<InputLabel id={labelId}>{label}</InputLabel>
			<Controller
				render={({ field }) => (
					<Select {...field} labelId={labelId} label={label}>
						{children}
					</Select>
				)}
				name={name}
				control={control}
				defaultValue={defaultValue}
			/>
		</FormControl>
	);
};

MaterialSelect.propTypes = {
	name: PropTypes.string.isRequired,
	label: PropTypes.string.isRequired,
	control: PropTypes.object.isRequired,
	defaultValue: PropTypes.any.isRequired,
	children: PropTypes.any.isRequired,
};

export default MaterialSelect;
