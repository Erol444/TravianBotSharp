import { Button, Grid, TextField, Typography } from "@mui/material";
import React from "react";
import { useForm } from "react-hook-form";

import ContentBox from "../../ContentBox";

const ActivitySettings = () => {
	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm();
	const onSubmit = (data) => console.log(data);
	return (
		<>
			<ContentBox>
				<Typography variant="h6">Acitvity Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={2}>
							<Typography variant="subtitle1">
								Working time:
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Min"
								variant="outlined"
								type="number"
								error={!!errors.work?.min}
								helperText={errors.work?.min?.message}
								{...register("work.min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Max"
								variant="outlined"
								type="number"
								error={!!errors.work?.max}
								helperText={errors.work?.max?.message}
								{...register("work.max")}
							/>
						</Grid>
						<Grid item xs={2}>
							<Typography variant="subtitle1">
								Sleeping time:
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Min"
								variant="outlined"
								type="number"
								error={!!errors.sleep?.min}
								helperText={errors.sleep?.min?.message}
								{...register("sleep.min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Max"
								variant="outlined"
								type="number"
								error={!!errors.sleep?.max}
								helperText={errors.sleep?.max?.message}
								{...register("sleep.max")}
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
export default ActivitySettings;
