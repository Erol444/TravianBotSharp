import React from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { Controller } from "react-hook-form";

import PropTypes from "prop-types";
import { nanoid } from "nanoid";

// src for who wonder what is this
// https://react-hook-form.com/advanced-usage#ControlledmixedwithUncontrolledComponents

const MUISelect = ({ name, label, control, options, ...props }) => {
	return (
		<FormControl {...props}>
			<InputLabel>{label}</InputLabel>
			<Controller
				render={({ field }) => (
					<Select {...field} label={label}>
						{options.length === 0 && (
							<MenuItem value="0">Loading . . .</MenuItem>
						)}
						{options.map((item) => {
							return (
								<MenuItem
									key={nanoid(10)}
									value={JSON.stringify(item.id)}
								>
									{item.name}
								</MenuItem>
							);
						})}
					</Select>
				)}
				name={name}
				control={control}
				defaultValue=""
			/>
		</FormControl>
	);
};

MUISelect.propTypes = {
	control: PropTypes.object.isRequired,
	label: PropTypes.string.isRequired,
	name: PropTypes.string.isRequired,
	options: PropTypes.array.isRequired,
};

export default MUISelect;
