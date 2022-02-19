import { Grid, TextField, Button } from "@mui/material";
import React from "react";
import { useForm } from "react-hook-form";

import ContentBox from "../../../../ContentBox";
import MUISwitch from "../../../../ref/MUISwitch";

const OptionBuildingModal = () => {
	const {
		register,
		control,
		handleSubmit,
		formState: { errors },
	} = useForm();

	const onSubmit = (data) => {
		console.log(data);
	};

	return (
		<>
			<ContentBox modal={true}>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={5}>
							<MUISwitch
								label="Instant upgrade"
								name="instant.active"
								control={control}
								fullWidth
							/>
						</Grid>
						<Grid item xs={7}>
							<TextField
								fullWidth
								label="When above x minutes"
								variant="outlined"
								type="number"
								error={!!errors.instant?.time}
								helperText={errors.instant?.time?.message}
								{...register("instant.time")}
							/>
						</Grid>
						<Grid item xs={12}>
							<Button
								type="submit"
								variant="contained"
								value="submit"
								color="success"
							>
								Save
							</Button>
						</Grid>
					</Grid>
				</form>
			</ContentBox>
		</>
	);
};

export default OptionBuildingModal;
